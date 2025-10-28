import { InputType, Field } from '@nestjs/graphql';

@InputType() // <-- no custom name here
export class LoginInput {
  @Field()
  email: string;

  @Field()
  password: string;
}
