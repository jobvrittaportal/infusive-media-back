import { Module } from '@nestjs/common';
import { RolesService } from './role.service';
import { RolesResolver } from './resolver/role.resolver';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Role } from './entitity/role.entity';
import { RolePermission } from './entitity/role-permission.entity';

@Module({
  imports: [TypeOrmModule.forFeature([Role, RolePermission])],
  providers: [RolesService, RolesResolver],
})
export class RoleModule {}
