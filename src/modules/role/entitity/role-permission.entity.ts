import { Entity, PrimaryGeneratedColumn, Column, ManyToOne } from 'typeorm';
import { ObjectType, Field, ID } from '@nestjs/graphql';
import { Role } from './role.entity';

@ObjectType()
class PermissionObject {
  @Field({ nullable: true })
  read: boolean;

  @Field({ nullable: true })
  upsert: boolean;

  @Field({ nullable: true })
  delete: boolean;
}

@ObjectType()
@Entity('role_permissions')
export class RolePermission {
  @Field(() => ID)
  @PrimaryGeneratedColumn('uuid')
  id: string;

  @Field()
  @Column()
  feature: string;

  @Field(() => PermissionObject)
  @Column('json')
  permissions: PermissionObject;

  @ManyToOne(() => Role, (role) => role.permissions, { onDelete: 'CASCADE' })
  role: Role;
}
