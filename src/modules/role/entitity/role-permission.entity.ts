import { Entity, PrimaryGeneratedColumn, Column, ManyToOne } from 'typeorm';
import { ObjectType, Field, ID } from '@nestjs/graphql';
import { Role } from './role.entity';

@ObjectType()
@Entity('role_permissions')
export class RolePermission {
  @Field(() => ID)
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Field()
  @Column()
  feature: string;

  @Field(() => String)
  @Column('json')
  permissions: Record<string, boolean>;

  @ManyToOne(() => Role, (role) => role.permissions, { onDelete: 'CASCADE' })
  role: Role;
}
