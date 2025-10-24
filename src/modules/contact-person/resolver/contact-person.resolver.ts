import { Resolver, Query, Mutation, Args } from '@nestjs/graphql';
import { ContactPerson } from '../entity/contact-person.entity';
import { ContactPersonService } from '../contact-person.service';
import { UseGuards } from '@nestjs/common';
import { GqlAuthGuard } from 'src/modules/auth/guards/jwt-auth.guard';
import { CurrentUser } from 'src/modules/auth/guards/current-user.guard';
// import { ContactPerson } from './contact-person.entity';
// import { ContactPersonService } from './contact-person.service';

@Resolver(() => ContactPerson)
export class ContactPersonResolver {
  constructor(private readonly contactService: ContactPersonService) {}

  @Query(() => [ContactPerson], { name: 'contactPersons' })
  findAll() {
    return this.contactService.findAll();
  }

  @UseGuards(GqlAuthGuard)
  @Mutation(() => ContactPerson)
  createContactPerson(
    @CurrentUser() user: any,
    @Args('name') name: string,
    @Args('email', { nullable: true }) email?: string,
    @Args('phone', { nullable: true }) phone?: string,
  ) {
    return this.contactService.create({ name, email, phone });
  }
}
