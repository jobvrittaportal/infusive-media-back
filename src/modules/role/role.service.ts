import { Injectable, NotFoundException } from '@nestjs/common';
import { Role } from './entitity/role.entity';
import { Repository } from 'typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { RolePermission } from './entitity/role-permission.entity';
import { CreateRoleInput } from './dto/create-role.input';
import { UpdateRoleInput } from './dto/update-role.input';

@Injectable()
export class RolesService {
  constructor(
    @InjectRepository(Role) private readonly roleRepo: Repository<Role>,
    @InjectRepository(RolePermission)
    private readonly permRepo: Repository<RolePermission>,
  ) {}

  // src/role/role.service.ts (or whatever path)
  async create(input: CreateRoleInput): Promise<Role> {
    const role = new Role();
    role.name = input.name;
    role.active = input.active ?? true;

    // safe mapping
    const perms = (input.permissions ?? []).map((p) => {
      const perm = new RolePermission();
      perm.feature = p.feature;
      try {
        perm.permissions = JSON.parse(p.permissions);
      } catch (err) {
        // If permissions isn't a JSON string, try to handle gracefully.
        // You might also want to throw a BadRequestException instead.
        perm.permissions =
          typeof p.permissions === 'string'
            ? ({} as Record<string, boolean>)
            : p.permissions;
      }
      return perm;
    });

    role.permissions = perms;
    return this.roleRepo.save(role);
  }

  async update(id: string, input: UpdateRoleInput): Promise<Role> {
    const role = await this.roleRepo.findOne({
      where: { id },
      relations: ['permissions'],
    });
    if (!role) throw new NotFoundException('Role not found');

    role.name = input.name ?? role.name;
    role.active = input.active ?? role.active;

    // remove old permissions only if you plan to replace them
    await this.permRepo.delete({ role: { id: role.id } });

    const perms = (input.permissions ?? []).map((p) => {
      const perm = new RolePermission();
      perm.feature = p.feature;
      try {
        perm.permissions = JSON.parse(p.permissions);
      } catch (err) {
        perm.permissions =
          typeof p.permissions === 'string'
            ? ({} as Record<string, boolean>)
            : p.permissions;
      }
      return perm;
    });

    role.permissions = perms;
    return this.roleRepo.save(role);
  }

  async findAll(): Promise<Role[]> {
    return this.roleRepo.find({ relations: ['permissions'] });
  }

  async findOne(id: string): Promise<Role> {
    const role = await this.roleRepo.findOne({
      where: { id },
      relations: ['permissions'],
    });
    if (!role) throw new NotFoundException('Role not found');
    return role;
  }

  async findByName(query: string): Promise<Role[]> {
  return this.roleRepo
    .createQueryBuilder('role')
    .where('LOWER(role.name) LIKE LOWER(:query)', { query: `%${query}%` })
    .getMany();
}
}
