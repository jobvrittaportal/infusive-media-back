import { InputType, Field, ID } from '@nestjs/graphql';
import { Type } from 'class-transformer';
import {
  IsArray,
  IsEmail,
  IsNotEmpty,
  IsOptional,
  Length,
  IsString,
  Matches,
} from 'class-validator';

@InputType()
export class CreateUserInput {
  @Field()
  @IsNotEmpty()
  firstName: string;

  @Field()
  @IsNotEmpty()
  lastName: string;

  @Field()
  @IsEmail()
  email: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsEmail()
  altEmail?: string;

  @Field()
  @IsNotEmpty()
  @Matches(/^[0-9]{10}$/, { message: 'Mobile must be a 10-digit number' })
  mobile: string;

  @Field({ nullable: true })
  @IsOptional()
  @Matches(/^[0-9]{10}$/, { message: 'Alt mobile must be a 10-digit number' })
  altMobile?: string;

  @Field({ nullable: true, defaultValue: 'user' })
  @IsOptional()
  userType?: string;

  // ✅ Instead of embedding CreateRoleInput, reference existing Role IDs
  @Field(() => [ID], { nullable: true })
  @IsOptional()
  @IsArray()
  @IsString({ each: true })
  roleIds?: string[];

  @Field()
  @IsNotEmpty()
  @Length(6, 100)
  password: string;

  @Field({ nullable: true })
  @IsOptional()
  createdBy?: string;

  @Field({ nullable: true })
  @IsOptional()
  updatedBy?: string;
}
