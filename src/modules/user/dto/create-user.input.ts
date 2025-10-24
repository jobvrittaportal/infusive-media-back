import { InputType, Field } from '@nestjs/graphql';
import { IsEmail, IsNotEmpty, IsOptional, Length } from 'class-validator';

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
  mobile: string;

  @Field({ nullable: true })
  @IsOptional()
  altMobile?: string;

  @Field({ nullable: true })
  @IsOptional()
  userType?: string;

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
