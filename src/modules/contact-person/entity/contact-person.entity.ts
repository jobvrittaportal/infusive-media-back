import { Field, Int, ObjectType } from '@nestjs/graphql';
import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@ObjectType() // GraphQL type
@Entity()
export class ContactPerson {
  @Field(() => Int)
  @PrimaryGeneratedColumn()
  id: number;

  @Field()
  @Column()
  name: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  email?: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  phone?: string;
}
