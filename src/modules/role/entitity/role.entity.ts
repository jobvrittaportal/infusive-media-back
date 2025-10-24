// roles.entity.ts
import { Entity, PrimaryGeneratedColumn, Column, ManyToMany, JoinTable } from 'typeorm';
// import { Permission } from '../permissions/permissions.entity';

@Entity('roles')
export class Role {
  @PrimaryGeneratedColumn()
  id: number;

  @Column({ unique: true })
  name: string; // e.g. "Admin", "Manager", "Employee"

//   @ManyToMany(() => Permission, { eager: true })
//   @JoinTable()
//   permissions: any[];
}
