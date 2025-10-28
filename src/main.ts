import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { GlobalGqlExceptionFilter } from './common/filters/http-exception.filters';
// import { HttpExceptionFilter } from './common/filters/http-exception.filters';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  // app.useGlobalFilters(new HttpExceptionFilter());
  app.useGlobalFilters(new GlobalGqlExceptionFilter());
  app.enableCors({
    origin: true,
    credentials: true,
  });
  await app.listen(process.env.PORT ?? 3000);
}
bootstrap();
