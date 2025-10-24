import { TypeOrmModuleOptions } from '@nestjs/typeorm';
import { Company } from 'src/modules/company/entities/company.entity';
// import { User } from 'src/modules/user/user.entity';
// import { User } from '../modules/user/user.entity';

export const typeOrmConfig: TypeOrmModuleOptions = {
  type: 'mysql',
  host: process.env.DB_HOST || 'localhost',
  port: parseInt(process.env.DB_PORT || '3306'),
  username: process.env.DB_USER || 'root',
  password: process.env.DB_PASS || 'admin',
  database: process.env.DB_NAME || 'infusive',
  entities: [__dirname + '/../**/*.entity.{ts,js}'],
  synchronize: true, // disable in prod
  logging: true,
};
