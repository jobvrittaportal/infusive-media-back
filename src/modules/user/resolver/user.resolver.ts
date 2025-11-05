import { Resolver, Query, Mutation, Args, ID } from '@nestjs/graphql';
import { UseGuards } from '@nestjs/common';
import { User } from '../entities/user.entity';
import { UserService } from '../user.service';
import { CreateUserInput } from '../dto/create-user.input';
import { GqlAuthGuard } from 'src/modules/auth/guards/jwt-auth.guard';
import { CurrentUser } from 'src/modules/auth/guards/current-user.guard';
import { UpdateUserInput } from '../dto/update-user.input';
import { ChangePasswordInput } from '../dto/change-password.input';

@Resolver(() => User)
export class UserResolver {
  constructor(private readonly userService: UserService) {}


  @Mutation(() => User, { name: 'registerUser' })
  async registerUser(@Args('input') input: CreateUserInput): Promise<User> {
    return this.userService.createUser(input);
  }


  @UseGuards(GqlAuthGuard)
  @Query(() => User, { name: 'userById' })
  async getProfile(@CurrentUser() user: any): Promise<User> {
    return this.userService.findById(user.id);
  }


  @UseGuards(GqlAuthGuard)
  @Query(() => [User], { name: 'users' })
  async getAllUsers(): Promise<User[]> {
    return this.userService.getAllUsers();
  }


  @UseGuards(GqlAuthGuard)
  @Query(() => User, { name: 'user' })
  async getUser(@Args('id', { type: () => ID }) id: string): Promise<User> {
    return this.userService.findById(id);
  }


  @UseGuards(GqlAuthGuard)
  @Mutation(() => User, { name: 'updateUser' })
  async updateUser(
    @Args('id', { type: () => ID }) id: string,
    @Args('input') input: UpdateUserInput,
  ): Promise<User> {
    return this.userService.updateUser(id, input);
  }


  @UseGuards(GqlAuthGuard)
  @Mutation(() => Boolean, { name: 'deleteUser' })
  async deleteUser(
    @Args('id', { type: () => ID }) id: string,
  ): Promise<boolean> {
    return this.userService.deleteUser(id);
  }


  @Mutation(() => Boolean, { name: 'changePassword' })
  async changePassword(
    @Args('input') input: ChangePasswordInput,
  ): Promise<boolean> {
    return this.userService.changePassword(input.userId, input.newPassword);
  }
}
