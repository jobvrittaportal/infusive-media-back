import { Module } from '@nestjs/common';
import { JwtModule } from '@nestjs/jwt';
import { PassportModule } from '@nestjs/passport';
import { AuthService } from './auth.service';
import { UserModule } from '../user/user.module';
import { AuthResolver } from './resolver/auth.resolver';
import { JwtStrategy } from './strategies/jwt.strategy';

@Module({
  imports: [
    UserModule,
    PassportModule,
    JwtModule.register({
      secret: process.env.JWT_SECRET || 'infusive@co2h2o',
      signOptions: { expiresIn: '7d' },
    }),
  ],
  providers: [AuthService, AuthResolver, JwtStrategy],
})
export class AuthModule {}
