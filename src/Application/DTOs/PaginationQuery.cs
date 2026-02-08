namespace Application.DTOs;

public  record PaginationQuery(int? Page = 1, int? PageSize = 10);
