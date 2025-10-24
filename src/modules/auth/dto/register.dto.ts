import { InputType, Field } from '@nestjs/graphql';

@InputType('user')
export class RegisterInput {
  @Field()
  name: string;

  @Field()
  email: string;

  @Field()
  password: string;
}
