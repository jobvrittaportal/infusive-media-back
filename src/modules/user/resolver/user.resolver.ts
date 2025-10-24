import { Resolver, Query, Mutation, Args, ID } from '@nestjs/graphql';
import { UseGuards } from '@nestjs/common';
import { User } from '../entities/user.entity';
import { UserService } from '../user.service';
import { CreateUserInput } from '../dto/create-user.input';
import { GqlAuthGuard } from 'src/modules/auth/guards/jwt-auth.guard';
import { CurrentUser } from 'src/modules/auth/guards/current-user.guard';
import { UpdateUserInput } from '../dto/update-user.input';

@Resolver(() => User)
export class UserResolver {
  constructor(private readonly userService: UserService) {}

  // ✅ Public registration (no guard)
  @Mutation(() => User, { name: 'registerUser' })
  async registerUser(@Args('input') input: CreateUserInput): Promise<User> {
    return this.userService.createUser(input);
  }

  // ✅ Protected query: fetch profile of logged-in user
  @UseGuards(GqlAuthGuard)
  @Query(() => User, { name: 'me' })
  async getProfile(@CurrentUser() user: any): Promise<User> {
    return this.userService.findById(user.id);
  }

  // ✅ Protected: fetch all users (admin-only if you want later)
  @UseGuards(GqlAuthGuard)
  @Query(() => [User], { name: 'users' })
  async getAllUsers(): Promise<User[]> {
    return this.userService.getAllUsers();
  }

  // ✅ Public query: fetch single user by id
  @Query(() => User, { name: 'user' })
  async getUser(@Args('id', { type: () => ID }) id: string): Promise<User> {
    return this.userService.findById(id);
  }

  // ✅ Protected mutation: update user info
  @UseGuards(GqlAuthGuard)
  @Mutation(() => User, { name: 'updateUser' })
  async updateUser(
    @Args('id', { type: () => ID }) id: string,
    @Args('input') input: UpdateUserInput,
  ): Promise<User> {
    return this.userService.updateUser(id, input);
  }

  // ✅ Protected mutation: delete user
  @UseGuards(GqlAuthGuard)
  @Mutation(() => Boolean, { name: 'deleteUser' })
  async deleteUser(
    @Args('id', { type: () => ID }) id: string,
  ): Promise<boolean> {
    return this.userService.deleteUser(id);
  }
}
