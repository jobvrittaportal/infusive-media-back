import { InputType, Field, Int, PartialType } from '@nestjs/graphql';
import { IsInt } from 'class-validator';
import { CreateCompanyDto } from './create-company.dto';

@InputType() // 👈 Required for GraphQL input
export class UpdateCompanyDto extends PartialType(CreateCompanyDto) {
  @Field(() => Int)
  @IsInt()
  id: number;
}
