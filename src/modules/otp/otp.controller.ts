import { Controller, Post, Body } from '@nestjs/common';
import { OtpService } from './otp.service';

@Controller('otp')
export class OtpController {
  constructor(private readonly otpService: OtpService) {}

  // POST /otp/send
  @Post('send')
  async sendOtp(@Body('email') email: string): Promise<boolean> {
     console.log("object", email)
    return this.otpService.sendOtp(email);
  }

  // POST /otp/verify
  @Post('verify')
  async verifyOtp(
    @Body('email') email: string,
    @Body('otp') otp: string,
  ): Promise<{userId?: string}> {
    const userIds =  this.otpService.verifyOtp(email, otp);
    console.log("object", userIds)
    return userIds;
  }
}
