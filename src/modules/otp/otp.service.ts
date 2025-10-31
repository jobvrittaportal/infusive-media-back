import { BadRequestException, Injectable, NotFoundException } from '@nestjs/common';
import { MailService } from '../mail/mail.service';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { User } from '../user/entities/user.entity';

@Injectable()
export class OtpService {
  private otpStore = new Map<string, { otp: string; expiresAt: number }>();

  constructor(
    private readonly mailService: MailService,
    @InjectRepository(User)
    private readonly userRepository: Repository<User>, // Inject User Repository
  ) {}

  // Send OTP
  async sendOtp(email: string): Promise<boolean> {
    const otp = Math.floor(100000 + Math.random() * 900000).toString();
    const expiresAt = Date.now() + 10 * 60 * 1000; // 10 minutes

    this.otpStore.set(email, { otp, expiresAt });
    await this.mailService.sendOtpEmail(email, otp);

    return true;
  }

  // Verify OTP and check email existence
  async verifyOtp(email: string, otp: string): Promise<{userId?: string }> {
    const record = this.otpStore.get(email);
    if (!record) throw new BadRequestException('No OTP found for this email');

    const isValid = record.otp === otp && record.expiresAt > Date.now();
    if (!isValid) throw new BadRequestException('Invalid or expired OTP');

    const user = await this.userRepository.findOne({
      where: { email },
      /* select: ['id'],  */
    });

    // Remove OTP after checking
    this.otpStore.delete(email);

    if (!user) {
      throw new NotFoundException('Email not registered');
    }
    
    return {
      userId: user.id,
    };
  }
}
