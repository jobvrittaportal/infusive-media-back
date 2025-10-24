import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
// import { CompanyService } from './company.service';
// import { CompanyResolver } from './company.resolver';
import { Company } from './entities/company.entity';
import { CompanyService } from './company.service';
import { CompanyController } from './company.controller';
import { CompanyResolver } from './resolver/company.resolver';

@Module({
  imports: [TypeOrmModule.forFeature([Company])],
  providers: [CompanyService, CompanyResolver],
  exports: [CompanyService],
  controllers: [CompanyController],
})
export class CompanyModule {}
