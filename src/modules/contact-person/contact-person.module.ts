import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';

import { ContactPersonService } from './contact-person.service';
import { ContactPerson } from './entity/contact-person.entity';
import { ContactPersonResolver } from './resolver/contact-person.resolver';

@Module({
  imports: [TypeOrmModule.forFeature([ContactPerson])],
  providers: [ContactPersonService, ContactPersonResolver],
})
export class ContactPersonModule {}
