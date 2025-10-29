import { Entity, PrimaryGeneratedColumn, Column, OneToMany } from 'typeorm';
import { ObjectType, Field, ID } from '@nestjs/graphql';
import { RolePermission } from './role-permission.entity';

@ObjectType()
@Entity('roles')
export class Role {
  @Field(() => ID)
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Field()
  @Column({ unique: true })
  name: string;

  @Field()
  @Column({ default: true })
  active: boolean;

  @Field(() => [RolePermission], { nullable: true })
  @OneToMany(() => RolePermission, (perm) => perm.role, {
    cascade: true,
    eager: true,
  })
  permissions: RolePermission[];
}
