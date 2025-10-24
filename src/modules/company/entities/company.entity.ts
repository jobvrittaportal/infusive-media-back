import { ObjectType, Field, ID } from '@nestjs/graphql';
import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  UpdateDateColumn,
} from 'typeorm';

@ObjectType()
@Entity('companies')
export class Company {
  @Field(() => ID)
  @PrimaryGeneratedColumn()
  id: number;

  @Field()
  @Column({ length: 150 })
  name: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 100 })
  industry_type?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 5 })
  phone_country_code?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 20 })
  phone_number?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, unique: true })
  email?: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  website_url?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 5 })
  country_code?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 50 })
  feid_gsd?: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  address?: string;

  @Field({ nullable: true })
  @Column({ nullable: true, length: 10 })
  postal_code?: string;

  @Field()
  @CreateDateColumn()
  createdAt: Date;

  @Field()
  @UpdateDateColumn()
  updatedAt: Date;
}
