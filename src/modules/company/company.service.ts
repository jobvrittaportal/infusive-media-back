import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

// import { UpdateCompanyInput } from './dto/update-company.input';
import { Company } from './entities/company.entity';
import { CreateCompanyDto } from './dto/create-company.dto';
import { UpdateCompanyDto } from './dto/update-company.dto';
import { applyFilters } from 'src/common/utils/apply-filter.util';

@Injectable()
export class CompanyService {
  constructor(
    @InjectRepository(Company)
    private readonly companyRepo: Repository<Company>,
  ) {}

  async create(input: CreateCompanyDto): Promise<Company> {
    const company = this.companyRepo.create(input);
    return this.companyRepo.save(company);
  }

  async findAll(filters: any): Promise<Company[]> {
    const query = this.companyRepo.createQueryBuilder();
    // applyFilters(query, 'company', filters, { exactFields: ['id'] });
    const result = await query.getMany();
    return result;
  }

  async findOne(id: number): Promise<Company> {
    const company = await this.companyRepo.findOne({ where: { id } });
    if (!company) throw new NotFoundException('Company not found');
    return company;
  }

  async update(input: UpdateCompanyDto): Promise<Company> {
    const company = await this.findOne(input.id);
    Object.assign(company, input);
    return this.companyRepo.save(company);
  }

  async remove(id: number): Promise<boolean> {
    const company = await this.findOne(id);
    await this.companyRepo.remove(company);
    return true;
  }
}
