import { Resolver, Mutation, Args, Query, ID } from '@nestjs/graphql';
import { RolesService } from '../role.service';
import { Role } from '../entitity/role.entity';
import { UpdateRoleInput } from '../dto/update-role.input';
import { CreateRoleInput } from '../dto/create-role.input';

@Resolver(() => Role)
export class RolesResolver {
  constructor(private readonly rolesService: RolesService) {}

  @Query(() => [Role])
  async roles(
    @Args('query', { type: () => String, nullable: true }) query: string,
  ) {
    if (query) {
      return this.rolesService.findByName(query);
    }
    return this.rolesService.findAll();
  }

  @Query(() => Role)
  async role(@Args('id') id: string) {
    return this.rolesService.findOne(id);
  }

  @Mutation(() => Role)
  async createRole(@Args('input') input: CreateRoleInput) {
    return this.rolesService.create(input);
  }

  @Mutation(() => Role)
  async updateRole(
    @Args('id', { type: () => ID }) id: string,
    @Args('input') input: UpdateRoleInput,
  ) : Promise<Role>{
    return this.rolesService.update(id, input);
  }
}
