import {
  Controller,
  Get,
  Post,
  Patch,
  Delete,
  Param,
  Body,
  ParseIntPipe,
  HttpCode,
  HttpStatus,
  UseGuards,
} from '@nestjs/common';
import { CompanyService } from './company.service';
import { CreateCompanyDto } from './dto/create-company.dto';
import { UpdateCompanyDto } from './dto/update-company.dto';
import { Company } from './entities/company.entity';
// import { JwtAuthGuard } from '../auth/guards/jwt-auth.guard';

@Controller('companies')
export class CompanyController {
  constructor(private readonly companyService: CompanyService) {}

  // CREATE company
  // @Post()
  // // @UseGuards(JwtAuthGuard)
  // @HttpCode(HttpStatus.CREATED)
  // create(@Body() createCompanyDto: CreateCompanyDto): Promise<Company> {
  //   return this.companyService.create(createCompanyDto);
  // }

  // GET all companies
  // @Get()
  // // @UseGuards(JwtAuthGuard)
  // findAll(): Promise<Company[]> {
  //   return this.companyService.findAll();
  // }

  // GET single company
  @Get(':id')
  // @UseGuards(JwtAuthGuard)
  findOne(@Param('id', ParseIntPipe) id: number): Promise<Company> {
    return this.companyService.findOne(id);
  }

  // UPDATE company
  @Patch(':id')
  // @UseGuards(JwtAuthGuard)
  update(
    @Param('id', ParseIntPipe) id: number,
    @Body() updateCompanyDto: UpdateCompanyDto,
  ): Promise<Company> {
    return this.companyService.update({ ...updateCompanyDto, id });
  }

  // DELETE company
  @Delete(':id')
  // @UseGuards(JwtAuthGuard)
  @HttpCode(HttpStatus.NO_CONTENT)
  async remove(@Param('id', ParseIntPipe) id: number): Promise<void> {
    await this.companyService.remove(id);
  }
}
