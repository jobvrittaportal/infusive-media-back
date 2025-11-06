import {
  Injectable,
  NotFoundException,
  BadRequestException,
} from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { ILike, In, Repository } from 'typeorm';
import * as bcrypt from 'bcrypt';
import { User } from './entities/user.entity';
import { CreateUserInput } from './dto/create-user.input';
import { UpdateUserInput } from './dto/update-user.input';
import { Role } from '../role/entitity/role.entity';
import { first } from 'rxjs';
import { lstatSync } from 'fs';

@Injectable()
export class UserService {
  constructor(
    @InjectRepository(User)
    private readonly userRepository: Repository<User>,
    @InjectRepository(Role)
    private readonly roleRepository: Repository<Role>,
  ) {}

  async createUser(input: CreateUserInput): Promise<User> {
    const existing = await this.userRepository.findOne({
      where: [{ email: input.email }, { mobile: input.mobile }],
    });
    if (existing)
      throw new BadRequestException('Email or Mobile already registered');

    const hashedPassword = await bcrypt.hash(input.password, 10);
    const user = this.userRepository.create({
      ...input,
      password: hashedPassword,
      userType: input.userType || 'user',
      roles: input.roleIds
        ? input.roleIds.map((roleId) => ({ id: roleId }))
        : [],
    });

    return this.userRepository.save(user);
  }


 /*  async getAllUsers(skip : number, limit : number, search?: string): Promise<User[]> {
    const where = search ? [
      { firstName: ILike(`%${search}%`) },
      { lastName: ILike(`%${search}%`) },
    ] : {};

    return this.userRepository.find({
      where,
      skip,
      take: limit,
      order: { createdAt: 'DESC' },
    });
  } */

 async getAllUsers( skip: number, limit: number, search?: string, joinRoles = false): Promise<User[]> {
    const query = this.userRepository
      .createQueryBuilder('user')
      .orderBy('user.createdAt', 'DESC')
      .skip(skip)
      .take(limit);
    if (search) {
      query.andWhere(
        `(user.firstName LIKE :search OR user.lastName LIKE :search OR CONCAT(user.firstName, ' ', user.lastName) LIKE :search)`,
        { search: `%${search}%` }
      );
    }

    if (joinRoles) {
      query.leftJoinAndSelect('user.roles', 'roles');
    }

    return await query.getMany();
  }

  async getTotalUsersCount(search?: string): Promise<number> {
    const query = this.userRepository
      .createQueryBuilder('user');

    if (search) {
      query.andWhere(
        `(user.firstName LIKE :search OR user.lastName LIKE :search OR CONCAT(user.firstName, ' ', user.lastName) LIKE :search)`,
        { search: `%${search}%` }
      );
    }
    return await query.getCount();
  }

  async findByEmail(email: string): Promise<User | null> {
    return this.userRepository.findOne({ where: { email } });
  }

  async findById(id: string): Promise<User> {
    const user = await this.userRepository.findOne({ where: { id } });
    if (!user) throw new NotFoundException('User not found');
    return user;
  }


async updateUser(id: string, input: UpdateUserInput): Promise<User> {
  const user = await this.findById(id);
  Object.assign(user, input, { updatedAt: new Date() });

  if (input.roleIds?.length) {
    const roles = await this.roleRepository.find({ where: { id: In(input.roleIds) } });
    user.roles = roles;
  } else {
    user.roles = [];
  }

  return await this.userRepository.save(user);
}


  async deleteUser(id: string): Promise<boolean> {
    const result = await this.userRepository.delete(id);

    return (result.affected ?? 0) > 0;
  }


  async changePassword(id: string, newPassword: string): Promise<boolean> {
    const user = await this.findById(id);
    if (!user) throw new NotFoundException('User not found');

    const hashed = await bcrypt.hash(newPassword, 10);
    user.password = hashed;
    user.updatedAt = new Date();

    await this.userRepository.save(user);
    return true;
  }
}
