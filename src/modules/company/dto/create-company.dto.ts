import { InputType, Field } from '@nestjs/graphql';
import { IsEmail, IsOptional, IsString, MaxLength } from 'class-validator';

@InputType() // 👈 Correct decorator
export class CreateCompanyDto {
  @Field() // 👈 Every exposed field must have this
  @IsString()
  @MaxLength(150)
  name: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(100)
  industry_type?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(5)
  phone_country_code?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(20)
  phone_number?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsEmail()
  email?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  website_url?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(5)
  country_code?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(50)
  feid_gsd?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  address?: string;

  @Field({ nullable: true })
  @IsOptional()
  @IsString()
  @MaxLength(10)
  postal_code?: string;
}
