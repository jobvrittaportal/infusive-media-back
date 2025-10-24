import { Resolver, Query, Mutation, Args, Int } from '@nestjs/graphql';
import { Company } from '../entities/company.entity';
import { CompanyService } from '../company.service';
import { CreateCompanyDto } from '../dto/create-company.dto';
import { UpdateCompanyDto } from '../dto/update-company.dto';
import { CompanyFilterDto } from '../dto/company-filter.dto';

@Resolver(() => Company)
export class CompanyResolver {
  constructor(private readonly companyService: CompanyService) {}

  // Fetch all companies
  @Query(() => [Company], { name: 'companies' })
  async findAll(
    @Args('filters', { nullable: true }) filters?: CompanyFilterDto,
  ) {
    return this.companyService.findAll(filters);
  }

  // Fetch a single company by ID
  @Query(() => Company, { name: 'company' })
  async findOne(@Args('id', { type: () => Int }) id: number) {
    return this.companyService.findOne(id);
  }

  // Create a company
  @Mutation(() => Company, { name: 'createCompany' })
  async createCompany(
    @Args('input') input: CreateCompanyDto,
  ): Promise<Company> {
    return this.companyService.create(input);
  }

  // Update a company
  @Mutation(() => Company, { name: 'updateCompany' })
  async updateCompany(
    @Args('input') input: UpdateCompanyDto,
  ): Promise<Company> {
    return this.companyService.update(input);
  }

  // 🔹 Delete a company
  @Mutation(() => Boolean, { name: 'removeCompany' })
  async removeCompany(
    @Args('id', { type: () => Int }) id: number,
  ): Promise<boolean> {
    return this.companyService.remove(id);
  }
}
