import {
  Injectable,
  UnauthorizedException,
  BadRequestException,
} from '@nestjs/common';
import { JwtService } from '@nestjs/jwt';
import * as bcrypt from 'bcrypt';
import { UserService } from '../user/user.service';
import { RegisterInput } from './dto/register.dto';
import { LoginInput } from './dto/login.dto';
import { CreateUserInput } from '../user/dto/create-user.input';

@Injectable()
export class AuthService {
  constructor(
    private readonly userService: UserService,
    private readonly jwtService: JwtService,
  ) {}

  // ✅ Register new user
  async register(input: CreateUserInput) {
    // Check if email or mobile already exists
    const existing = await this.userService.findByEmail(input.email);
    if (existing) throw new BadRequestException('Email already registered');

    const user = await this.userService.createUser(input); // createUser already hashes password
    const token = this.jwtService.sign({ sub: user.id });

    return { access_token: token, user };
  }

  // ✅ Login existing user
  async login(input: LoginInput) {
    const user = await this.userService.findByEmail(input.email);
    if (!user) throw new UnauthorizedException('User not found');

    const isValid = await bcrypt.compare(input.password, user.password);
    if (!isValid) throw new UnauthorizedException('Invalid credentials');

    const token = this.jwtService.sign({ sub: user.id });

    return { access_token: token, user };
  }

  // ✅ Optional: validate JWT payload (used in guards)
  async validateUser(userId: string): Promise<any> {
    const user = await this.userService.findById(userId);
    if (!user) throw new UnauthorizedException('User not found');
    return user;
  }
}
