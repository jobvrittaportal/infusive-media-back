import { Injectable } from '@nestjs/common';
import * as nodemailer from 'nodemailer';

@Injectable()
export class MailService {
  private transporter;

  constructor() {
    this.transporter = nodemailer.createTransport({
      service: 'gmail',
      auth: {
        user: process.env.EMAIL_USER, // your sender email
        pass: process.env.EMAIL_PASS, // your app password
      },
    });
  }

  async sendOtpEmail(email: string, otp: string) {
    const mailOptions = {
      from: `"Infusive Media" <${process.env.EMAIL_USER}>`,
      to: email,
      subject: 'Your OTP for Password Reset',
      html: `
        <p>Hello,</p>
        <p>Your OTP for resetting password is: <b>${otp}</b></p>
        <p>This OTP is valid for 10 minutes.</p>
      `,
    };

    await this.transporter.sendMail(mailOptions);
  }
}
