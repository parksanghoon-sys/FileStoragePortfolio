# ☁️ Cloud File Storage Platform

**ASP.NET Core + React 기반의 파일 스토리지 플랫폼**

사용자는 파일(사진, 동영상 등)을 업로드하고, 공유 링크를 통해 특정 사용자에게 접근 권한을 부여할 수 있습니다. 공유 승인 시 SignalR을 통해 실시간 알림을 받고, 댓글 기능도 제공됩니다. 슬라이딩 Refresh Token 기반 인증 구조와 역할 기반 권한 관리를 포함하고 있습니다.

마이크로 서비스 아키텍쳐

파일 업로드 및 다운로드 서비스 + 인증 서비스 + 클라이언트(사진 및 동영상 미리보기 기능)

---

## 📚 기술 스택

| 영역         | 기술                                               |
| ------------ | -------------------------------------------------- |
| Frontend     | React, TypeScript, Vite, Tailwind CSS              |
| Backend API  | ASP.NET Core Web API (.NET 7)                      |
| 인증         | Duende IdentityServer, JWT, RefreshToken (Sliding) |
| 실시간       | SignalR                                            |
| 데이터베이스 | PostgreSQL                                         |
| 캐시         | Redis                                              |
| 배포         | Docker, Docker Compose                             |
| 기타         | Swagger, Serilog, FluentValidation                 |

---

## 🏗️ 시스템 아키텍처

인증서비스 + 파일서비스

React SPA
↓
API Gateway (YARP)
↓
┌────────────────────────────┬────────────────────────────┬────────────────────┐
│ Identity Server            │ File Service API           │ Notification Hub   │
│ (JWT + RefreshToken + Redis) │ (Upload, Share, Download) │ (SignalR)          │
└────────────────────────────┴────────────────────────────┴────────────────────┘

````

---

## 🔐 인증 및 권한

- JWT 기반 인증 + Sliding Refresh Token
- Redis를 이용한 RefreshToken 저장 및 만료 관리
- 역할 기반 권한 (Admin, User, Guest)

---

## 📦 주요 기능

### ✅ 파일 관리
- Drag & Drop 또는 멀티 파일 업로드
- 메타 정보는 DB, 실제 파일은 로컬 또는 클라우드 저장소
- 다운로드 기능

### 🔗 공유 기능
- 공유 링크 생성 (GUID + 암호화)
- 이메일 전송
- 수신자가 링크 승인 시, 해당 파일 Read 권한 부여

### 📢 실시간 알림
- 공유 승인 시 SignalR을 통해 알림 푸시
- 댓글 작성 시 알림 전송

### 💬 댓글 시스템
- 파일 단위 댓글 등록, 수정, 삭제
- 실시간 댓글 반영

---

## 🧪 API 문서 (Swagger)

- 인증/인가 포함된 Swagger UI 제공
- 자동화된 API 명세 (OpenAPI)
- `/swagger/index.html` 접근 가능

---

## 🐳 Docker 배포

### 요구 사항
- Docker
- Docker Compose

### 실행 방법
```bash
git clone https://github.com/your-repo/file-storage-platform.git
cd file-storage-platform
docker-compose up --build
````

### 주요 서비스 포트

| 서비스         | 포트         |
| -------------- | ------------ |
| Client (React) | 3000         |
| API Gateway    | 5000         |
| IdentityServer | 5001         |
| File API       | 5002         |
| Redis          | 6379         |
| DB             | 5432 or 1433 |

---

## 🧑‍💻 프로젝트 구조 (Clean Architecture)

```
src/
  ├── SharedKernel/         # 공통 엔티티 및 유틸리티
  ├── IdentityService/      # 인증 서버
  ├── FileService/          # 파일 업로드, 공유, 다운로드
  ├── NotificationService/  # SignalR Hub
  ├── ApiGateway/           # YARP 기반 Gateway
  └── ClientApp/            # React 클라이언트
```

---

## 📎 예시 화면 (선택)

> 추후 이미지 삽입 예:

* 파일 업로드 화면
* 공유 링크 승인 UI
* 댓글 작성 및 실시간 반영
* 관리자 관리 페이지 등

---

## 📌 향후 개선 방향

* S3 등 외부 저장소 연동
* 사용자별 스토리지 용량 설정
* 이메일 인증/2FA
* 다국어 UI 지원

---

# 파일 스토리지 포토폴리오 프로젝트 진행 계획

## 🏗️ 프로젝트 아키텍처 개요

### 백엔드 구조 (클린 아키텍처)

```
FileStoragePortfolio/
├── src/
│   ├── FileStorage.Domain/           # 엔티티, 도메인 로직, 인터페이스
│   ├── FileStorage.Application/      # 유즈케이스, 서비스, DTOs
│   ├── FileStorage.Infrastructure/   # 데이터베이스, 외부 서비스, 리포지토리
│   ├── FileStorage.WebAPI/          # 컨트롤러, 미들웨어, SignalR
│   └── FileStorage.Shared/          # 공통 클래스, 유틸리티
├── tests/
├── docker/
└── docs/
```

### 프론트엔드 구조

```
client/
├── src/
│   ├── components/
│   │   ├── common/
│   │   ├── auth/
│   │   ├── files/
│   │   ├── notifications/
│   │   └── comments/
│   ├── pages/
│   ├── services/
│   ├── store/
│   ├── utils/
│   └── types/
├── public/
└── package.json
```

## 📋 단계별 개발 계획

### Phase 1: 기반 설정 및 공통 라이브러리 (1-2주)

#### 1.1 프로젝트 구조 생성

```bash
# 솔루션 생성
dotnet new sln -n FileStoragePortfolio

# 프로젝트 생성
dotnet new classlib -n FileStorage.Domain
dotnet new classlib -n FileStorage.Application
dotnet new classlib -n FileStorage.Infrastructure
dotnet new webapi -n FileStorage.WebAPI
dotnet new classlib -n FileStorage.Shared

# React 프로젝트
npx create-react-app client --template typescript
```

#### 1.2 공통 라이브러리 구현 (FileStorage.Shared)

```csharp
// BaseEntity.cs
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}

// IRepository.cs
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<IQueryable<T>> GetQueryable();
}

// ApiResponse.cs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

// 페이징
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
```

#### 1.3 도메인 엔티티 설계

```csharp
// User.cs
public class User : BaseEntity
{
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public List<FileEntity> Files { get; set; } = new();
}

// FileEntity.cs
public class FileEntity : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public string ContentType { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public List<FileShare> Shares { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}

// FileShare.cs
public class FileShare : BaseEntity
{
    public int FileId { get; set; }
    public FileEntity File { get; set; }
    public string SharedWithEmail { get; set; }
    public string ShareToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public bool IsAccepted { get; set; }
    public SharePermission Permission { get; set; } = SharePermission.Read;
}

// Comment.cs
public class Comment : BaseEntity
{
    public string Content { get; set; }
    public int FileId { get; set; }
    public FileEntity File { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int? ParentCommentId { get; set; }
    public Comment ParentComment { get; set; }
    public List<Comment> Replies { get; set; } = new();
}

// Enums
public enum UserRole { Admin, User, Guest }
public enum SharePermission { Read, ReadWrite }
```

### Phase 2: 인증 시스템 구현 (1-2주)

#### 2.1 JWT + RefreshToken 서비스

```csharp
// IAuthService.cs
public interface IAuthService
{
    Task<ApiResponse<AuthResult>> LoginAsync(LoginRequest request);
    Task<ApiResponse<AuthResult>> RegisterAsync(RegisterRequest request);
    Task<ApiResponse<AuthResult>> RefreshTokenAsync(string refreshToken);
    Task<ApiResponse<bool>> RevokeTokenAsync(string refreshToken);
}

// AuthService.cs (슬라이딩 토큰 구현)
public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
  
    public async Task<ApiResponse<AuthResult>> RefreshTokenAsync(string refreshToken)
    {
        // 슬라이딩 토큰 로직
        // 1. RefreshToken 검증
        // 2. 새로운 AccessToken 발급
        // 3. RefreshToken 연장 (sliding)
        // 4. 오래된 토큰 정리
    }
}
```

#### 2.2 미들웨어 구현

```csharp
// JwtMiddleware.cs
public class JwtMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
            await AttachUserToContext(context, token);
        
        await next(context);
    }
}
```

### Phase 3: 파일 스토리지 핵심 기능 (2-3주)

#### 3.1 파일 업로드 서비스

```csharp
// IFileService.cs
public interface IFileService
{
    Task<ApiResponse<FileUploadResult>> UploadFilesAsync(IFormFileCollection files, int userId);
    Task<ApiResponse<Stream>> DownloadFileAsync(int fileId, int userId);
    Task<ApiResponse<bool>> DeleteFileAsync(int fileId, int userId);
    Task<ApiResponse<PagedResult<FileDto>>> GetUserFilesAsync(int userId, int page, int size);
}

// FileService.cs
public class FileService : IFileService
{
    public async Task<ApiResponse<FileUploadResult>> UploadFilesAsync(IFormFileCollection files, int userId)
    {
        var results = new List<FileDto>();
    
        foreach (var file in files)
        {
            // 파일 검증
            if (!IsValidFile(file))
                continue;
            
            // 고유 파일명 생성
            var fileName = GenerateUniqueFileName(file.FileName);
            var filePath = Path.Combine(_uploadPath, fileName);
        
            // 파일 저장
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        
            // 데이터베이스 저장
            var fileEntity = new FileEntity
            {
                FileName = file.FileName,
                FilePath = filePath,
                FileSize = file.Length,
                ContentType = file.ContentType,
                UserId = userId
            };
        
            await _fileRepository.AddAsync(fileEntity);
            results.Add(_mapper.Map<FileDto>(fileEntity));
        }
    
        return new ApiResponse<FileUploadResult> 
        { 
            Success = true, 
            Data = new FileUploadResult { Files = results } 
        };
    }
}
```

#### 3.2 컨트롤러 구현

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles(IFormFileCollection files)
    {
        var userId = GetCurrentUserId();
        var result = await _fileService.UploadFilesAsync(files, userId);
    
        if (result.Success)
            return Ok(result);
        
        return BadRequest(result);
    }
  
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _fileService.DownloadFileAsync(id, userId);
    
        if (result.Success)
            return File(result.Data, "application/octet-stream");
        
        return NotFound();
    }
}
```

### Phase 4: 공유 및 권한 시스템 (2-3주)

#### 4.1 파일 공유 서비스

```csharp
public interface IFileShareService
{
    Task<ApiResponse<string>> CreateShareLinkAsync(int fileId, string email, int userId);
    Task<ApiResponse<bool>> AcceptShareAsync(string shareToken);
    Task<ApiResponse<bool>> RejectShareAsync(string shareToken);
    Task<ApiResponse<List<SharedFileDto>>> GetSharedFilesAsync(int userId);
}

public class FileShareService : IFileShareService
{
    public async Task<ApiResponse<string>> CreateShareLinkAsync(int fileId, string email, int userId)
    {
        // 파일 소유권 확인
        var file = await _fileRepository.GetByIdAsync(fileId);
        if (file.UserId != userId)
            return new ApiResponse<string> { Success = false, Message = "권한이 없습니다." };
        
        // 공유 토큰 생성
        var shareToken = Guid.NewGuid().ToString();
        var share = new FileShare
        {
            FileId = fileId,
            SharedWithEmail = email,
            ShareToken = shareToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };
    
        await _shareRepository.AddAsync(share);
    
        // 이메일 발송
        await _emailService.SendShareInvitationAsync(email, shareToken, file.FileName);
    
        return new ApiResponse<string> { Success = true, Data = shareToken };
    }
}
```

### Phase 5: 실시간 알림 시스템 (1-2주)

#### 5.1 SignalR Hub

```csharp
[Authorize]
public class NotificationHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
  
    public async Task LeaveUserGroup(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}

// INotificationService.cs
public interface INotificationService
{
    Task SendFileShareNotificationAsync(int userId, string message);
    Task SendCommentNotificationAsync(int userId, string message);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
  
    public async Task SendFileShareNotificationAsync(int userId, string message)
    {
        await _hubContext.Clients.Group($"user_{userId}")
            .SendAsync("FileShareAccepted", new { message, timestamp = DateTime.UtcNow });
    }
}
```

#### 5.2 React SignalR 클라이언트

```typescript
// signalr.service.ts
class SignalRService {
    private connection: HubConnection;
  
    constructor() {
        this.connection = new HubConnectionBuilder()
            .withUrl("/notificationHub", {
                accessTokenFactory: () => localStorage.getItem('accessToken') || ""
            })
            .build();
    }
  
    async start() {
        await this.connection.start();
        const userId = getCurrentUserId();
        await this.connection.invoke("JoinUserGroup", userId.toString());
    }
  
    onFileShareAccepted(callback: (data: any) => void) {
        this.connection.on("FileShareAccepted", callback);
    }
}
```

### Phase 6: 댓글 시스템 (1-2주)

#### 6.1 댓글 서비스

```csharp
public interface ICommentService
{
    Task<ApiResponse<CommentDto>> AddCommentAsync(AddCommentRequest request, int userId);
    Task<ApiResponse<List<CommentDto>>> GetFileCommentsAsync(int fileId);
    Task<ApiResponse<bool>> DeleteCommentAsync(int commentId, int userId);
}
```

### Phase 7: React 프론트엔드 구현 (2-3주)

#### 7.1 상태 관리 (Redux Toolkit)

```typescript
// store/auth.slice.ts
interface AuthState {
    user: User | null;
    accessToken: string | null;
    isAuthenticated: boolean;
    loading: boolean;
}

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        loginSuccess: (state, action) => {
            state.user = action.payload.user;
            state.accessToken = action.payload.accessToken;
            state.isAuthenticated = true;
        },
        logout: (state) => {
            state.user = null;
            state.accessToken = null;
            state.isAuthenticated = false;
        }
    }
});
```

#### 7.2 파일 업로드 컴포넌트

```typescript
// components/files/FileUpload.tsx
const FileUpload: React.FC = () => {
    const [files, setFiles] = useState<File[]>([]);
    const [uploadProgress, setUploadProgress] = useState<Record<string, number>>({});
  
    const handleDrop = useCallback((acceptedFiles: File[]) => {
        setFiles(prev => [...prev, ...acceptedFiles]);
    }, []);
  
    const uploadFiles = async () => {
        const formData = new FormData();
        files.forEach(file => formData.append('files', file));
    
        try {
            const response = await fileService.uploadFiles(formData, {
                onUploadProgress: (progressEvent) => {
                    // 진행률 업데이트
                }
            });
        
            // 성공 처리
        } catch (error) {
            // 에러 처리
        }
    };
  
    return (
        <div {...getRootProps()}>
            <input {...getInputProps()} />
            {/* UI 컴포넌트 */}
        </div>
    );
};
```

### Phase 8: Docker 배포 설정 (1주)

#### 8.1 docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: src/FileStorage.WebAPI/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=filestorage;Username=postgres;Password=password
    depends_on:
      - db
      - redis
    volumes:
      - ./uploads:/app/uploads

  client:
    build:
      context: ./client
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - api

  db:
    image: postgres:13
    environment:
      POSTGRES_DB: filestorage
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: password
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  redis:
    image: redis:6-alpine
    ports:
      - "6379:6379"

volumes:
  postgres_data:
```

## 🚀 프로젝트 실행 순서

1. **백엔드 설정**
   ```bash
   cd src/FileStorage.WebAPI
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
2. **프론트엔드 설정**
   ```bash
   cd client
   npm install
   npm start
   ```
3. **Docker 실행**
   ```bash
   docker-compose up -d
   ```

## 📝 주요 고려사항

### 보안

* JWT 토큰 보안 (HttpOnly 쿠키 사용 고려)
* 파일 업로드 검증 (파일 타입, 크기 제한)
* Path Traversal 공격 방지
* CORS 정책 설정

### 성능

* 파일 청크 업로드 구현
* 데이터베이스 인덱싱
* Redis 캐싱 활용
* 이미지 썸네일 생성

### 확장성

* 클라우드 스토리지 연동 준비 (AWS S3, Azure Blob)
* 마이크로서비스 아키텍처 고려
* API 버전 관리

## ⏱️ 예상 개발 기간

**총 10-14주 (2.5-3.5개월)**

각 Phase는 병렬로 진행 가능한 부분들이 있어 전체 기간을 단축할 수 있습니다.

## 🎯 성공 지표

* [ ] 사용자 인증 및 권한 관리
* [ ] 파일 업로드/다운로드 기능
* [ ] 실시간 알림 시스템
* [ ] 파일 공유 및 승인 시스템
* [ ] 댓글 기능
* [ ] 반응형 웹 디자인
* [ ] Docker 배포 환경
