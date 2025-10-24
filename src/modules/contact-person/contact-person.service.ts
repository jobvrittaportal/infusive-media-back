import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { ContactPerson } from './entity/contact-person.entity';

@Injectable()
export class ContactPersonService {
  constructor(
    @InjectRepository(ContactPerson)
    private contactRepo: Repository<ContactPerson>,
  ) {}

  findAll(): Promise<ContactPerson[]> {
    return this.contactRepo.find();
  }

  create(data: Partial<ContactPerson>): Promise<ContactPerson> {
    const person = this.contactRepo.create(data);
    return this.contactRepo.save(person);
  }
}
