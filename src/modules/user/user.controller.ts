// import { Body, Controller, Get, Param, Post, UseGuards } from '@nestjs/common';
// // import { UsersService } from './user.service';
// // import { AuthGuard } from 'src/guards/auth/auth.guard';
// import { UserService } from './user.service';
// // import { JwtAuthGuard } from '../auth/guards/jwt-auth.guard';

// @Controller('users')
// export class UserController {
//   constructor(private readonly userService: UserService) {}

//   // @Post()
//   // create(@Body() body: any) {
//   //   return this.userService.createUser(body.name, body.email, body.password);
//   // }

//   // @Get()
//   // findAll() {
//   //   return this.usersService.findAll();
//   // }

//   @Get()
//   // @UseGuards(JwtAuthGuard)
//   getUers() {
//     return this.userService.getAllUsers();
//   }
//   // @Get(':id')
//   // getUersByid(@Param('id') id: string) {
//   //   return this.userService.getAllUsersId(+id);
//   // }
// }
