import { InputType, Field } from '@nestjs/graphql';

@InputType('user')
export class LoginInput {
  @Field()
  email: string;

  @Field()
  password: string;
}
