import { Module } from '@nestjs/common';
import { UserService } from './user.service';
// import { UserController } from './user.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { User } from './entities/user.entity';
// import { UserService } from './user.service';
import { UserResolver } from './resolver/user.resolver';

@Module({
  imports: [TypeOrmModule.forFeature([User])],
  providers: [UserService, UserResolver],
  controllers: [],
  exports: [UserService],
})
export class UserModule {}
