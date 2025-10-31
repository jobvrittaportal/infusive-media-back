import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { TypeOrmModule } from '@nestjs/typeorm';
import { typeOrmConfig } from './config/typeorm.config';
import { ConfigModule } from '@nestjs/config';
import { CompanyModule } from './modules/company/company.module';
import { CompanyController } from './modules/company/company.controller';
import { ContactPersonModule } from './modules/contact-person/contact-person.module';
import { UserModule } from './modules/user/user.module';
import { AuthModule } from './modules/auth/auth.module';
import { RoleModule } from './modules/role/role.module';
import { GraphQLModule } from '@nestjs/graphql';
import { ApolloDriver, ApolloDriverConfig } from '@nestjs/apollo';
import { join } from 'path';
import { graphqlConfig } from './config/graphql.config';
import { OtpModule } from './modules/otp/otp.module';
import { MailModule } from './modules/mail/mail.module';

@Module({
  imports: [
    ConfigModule.forRoot({ isGlobal: true }),
    TypeOrmModule.forRoot(typeOrmConfig),
    GraphQLModule.forRoot(graphqlConfig),
    CompanyModule,
    ContactPersonModule,
    UserModule,
    AuthModule,
    RoleModule,
    OtpModule,
    MailModule
  ],
  controllers: [AppController, CompanyController],
  providers: [AppService],
})
export class AppModule {}
