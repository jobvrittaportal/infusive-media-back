// src/role/dto/create-role.input.ts
import { InputType, Field } from '@nestjs/graphql';
import GraphQLJSON from 'graphql-type-json';

@InputType()
class PermissionPairInput {
  @Field()
  feature: string;

  // the client currently sends a JSON string for permissions
  @Field(() => String)
  permissions: string;
}

@InputType()
export class CreateRoleInput {
  @Field()
  name: string;

  @Field({ defaultValue: true })
  active: boolean;

  // Make permissions optional
  @Field(() => GraphQLJSON, { nullable: true })
  permissions?: any;
}
