import {
  Catch,
  ArgumentsHost,
  HttpException,
  ExecutionContext,
} from '@nestjs/common';
import {
  GqlExceptionFilter,
  GqlContextType,
  GqlExecutionContext,
} from '@nestjs/graphql';
import { GraphQLError } from 'graphql';

@Catch()
export class GlobalGqlExceptionFilter implements GqlExceptionFilter {
  catch(exception: any, host: ExecutionContext) {
    if (host.getType<GqlContextType>() === 'graphql') {
      const gqlHost = GqlExecutionContext.create(host);
    }
    // Handle standard HttpExceptions
    if (exception instanceof HttpException) {
      const response = exception.getResponse();
      const status =
        typeof response === 'object' && 'statusCode' in response
          ? (response as any).statusCode
          : (exception.getStatus?.() ?? 500);

      const message =
        typeof response === 'object' && 'message' in response
          ? (response as any).message
          : exception.message;

      return new GraphQLError(message, {
        extensions: {
          code: status,
          timestamp: new Date().toISOString(),
        },
      });
    }

    // Handle other unknown/unexpected errors
    return new GraphQLError('Internal server error', {
      extensions: {
        code: 'INTERNAL_SERVER_ERROR',
        message: exception.message || 'Unexpected error occurred',
        timestamp: new Date().toISOString(),
      },
    });
  }
}
