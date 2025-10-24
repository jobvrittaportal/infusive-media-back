import { Resolver, Mutation, Args } from '@nestjs/graphql';
import { AuthService } from '../auth.service';
import { AuthResponse } from '../models/auth.model';
import { RegisterInput } from '../dto/register.dto';
import { LoginInput } from '../dto/login.dto';
import { CreateUserInput } from 'src/modules/user/dto/create-user.input';

@Resolver()
export class AuthResolver {
  constructor(private readonly authService: AuthService) {}

  @Mutation(() => AuthResponse)
  register(@Args('input') input: CreateUserInput) {
    return this.authService.register(input);
  }

  @Mutation(() => AuthResponse)
  login(@Args('input') input: LoginInput) {
    return this.authService.login(input);
  }
}
