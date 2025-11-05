import { Module } from '@nestjs/common';
import { UserService } from './user.service';
import { TypeOrmModule } from '@nestjs/typeorm';
import { User } from './entities/user.entity';
import { UserResolver } from './resolver/user.resolver';
import { RoleModule } from '../role/role.module';
import { Role } from '../role/entitity/role.entity';

@Module({
  imports: [TypeOrmModule.forFeature([User, Role]),RoleModule],
  providers: [UserService, UserResolver],
  controllers: [],
  exports: [UserService, TypeOrmModule, RoleModule],
})
export class UserModule {}
