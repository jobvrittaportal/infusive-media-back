import { SelectQueryBuilder, ObjectLiteral } from 'typeorm';

interface FilterOptions {
  exactFields?: string[];
}

export function applyFilters<T extends ObjectLiteral>(
  query: SelectQueryBuilder<T>,
  alias: string,
  filters: Record<string, any> = {},
  options: FilterOptions = {},
): SelectQueryBuilder<T> {
  const { exactFields = [] } = options;

  Object.entries(filters).forEach(([key, value]) => {
    if (value === undefined || value === null || value === '') return;

    if (exactFields.includes(key)) {
      query.andWhere(`${alias}.${key} = :${key}`, { [key]: value });
    } else if (typeof value === 'string') {
      query.andWhere(`${alias}.${key} LIKE :${key}`, { [key]: `%${value}%` });
    } else {
      query.andWhere(`${alias}.${key} = :${key}`, { [key]: value });
    }
  });

  return query;
}
