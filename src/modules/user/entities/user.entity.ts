import { ObjectType, Field, ID } from '@nestjs/graphql';
import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  UpdateDateColumn,
  ManyToMany,
  JoinTable,
} from 'typeorm';
// import { Role } from 'src/modules/roles/role.entity'; // assuming you’ll have a Role module later

@ObjectType()
@Entity('users')
export class User {
  @Field(() => ID)
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Field()
  @Column()
  firstName: string;

  @Field()
  @Column()
  lastName: string;

  @Field()
  @Column({ unique: true })
  email: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  altEmail?: string;

  @Field()
  @Column()
  mobile: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  altMobile?: string;

  @Field()
  @Column({ default: 'user' })
  userType: string; // e.g. "admin", "agent", "tenant", etc.

  @Column()
  password: string; // DO NOT expose this via @Field — keep it hidden

  // @Field(() => [Role], { nullable: true })
  // @ManyToMany(() => Role, { eager: true })
  // @JoinTable({
  //   name: 'user_roles',
  //   joinColumn: { name: 'user_id', referencedColumnName: 'id' },
  //   inverseJoinColumn: { name: 'role_id', referencedColumnName: 'id' },
  // })
  // roles?: Role[];

  @Field({ nullable: true })
  @Column({ nullable: true })
  createdBy?: string;

  @Field({ nullable: true })
  @Column({ nullable: true })
  updatedBy?: string;

  @Field()
  @CreateDateColumn()
  createdAt: Date;

  @Field()
  @UpdateDateColumn()
  updatedAt: Date;
}
