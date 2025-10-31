import { Module } from '@nestjs/common';
import { OtpService } from './otp.service';
import { OtpController } from './otp.controller';
import { MailModule } from '../mail/mail.module';
import { UserModule } from '../user/user.module';

@Module({
  imports: [MailModule, UserModule],
  providers: [OtpService],
  controllers: [OtpController],
})
export class OtpModule {}
