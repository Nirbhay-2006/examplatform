# Backend Anti-Cheating API Module

## Functional Flow
1. Student starts exam with `POST /api/exam/start-session`.
2. Backend creates a new `exam_sessions` row with `status=active`.
3. Frontend continuously sends behavior telemetry via `POST /api/exam/monitoring-event`.
4. If frontend detects cheating, it calls `POST /api/exam/log-violation`.
5. Backend stores the violation in `violation_logs`, marks session as violated, and auto-submits instantly.
6. Auto-submit stores submission snapshot in `exam_submissions`, sets session `status=submitted`, sets `access_locked_at`, and blocks further access.
7. Admin reviews incidents with `GET /api/exam/violations`.

## REST APIs

### POST /api/exam/start-session
Request:
```json
{
  "user_id": 101,
  "exam_id": 5001,
  "device_info": "Chrome 132.0 / Windows 11",
  "ip_address": "49.37.120.10"
}
```

Response:
```json
{
  "session_id": "8f5a52a8-969d-4b84-a4ec-f3f3ef8b78c9",
  "start_time": "2026-03-05T07:45:00Z",
  "session_status": "active"
}
```

### POST /api/exam/log-violation
Request:
```json
{
  "session_id": "8f5a52a8-969d-4b84-a4ec-f3f3ef8b78c9",
  "violation_type": "tab_switch"
}
```

Response:
```json
{
  "violation_logged": true,
  "action_taken": "auto_submitted"
}
```

### POST /api/exam/auto-submit
Request:
```json
{
  "session_id": "8f5a52a8-969d-4b84-a4ec-f3f3ef8b78c9",
  "reason": "Violation detected: tab_switch",
  "answers_snapshot": "{\"q1\":\"A\",\"q2\":\"B\"}"
}
```

Response:
```json
{
  "submitted": true,
  "session_status": "submitted",
  "submitted_at": "2026-03-05T08:00:15Z",
  "reason": "Violation detected: tab_switch"
}
```

### GET /api/exam/session/{session_id}
Response:
```json
{
  "session_id": "8f5a52a8-969d-4b84-a4ec-f3f3ef8b78c9",
  "user_id": 101,
  "exam_id": 5001,
  "start_time": "2026-03-05T07:45:00Z",
  "end_time": "2026-03-05T08:00:15Z",
  "status": "submitted",
  "is_violated": true,
  "violation_reason": "Violation detected: tab_switch",
  "ip_address": "49.37.120.10",
  "device_info": "Chrome 132.0 / Windows 11"
}
```

### GET /api/exam/violations?exam_id=5001&user_id=101&date=2026-03-05&page=1&page_size=50
Response:
```json
[
  {
    "id": 1,
    "session_id": "8f5a52a8-969d-4b84-a4ec-f3f3ef8b78c9",
    "user_id": 101,
    "exam_id": 5001,
    "violation_type": "tab_switch",
    "timestamp": "2026-03-05T08:00:14Z",
    "action_taken": "auto_submitted"
  }
]
```

## SQL Schema (Code-First Equivalent)
```sql
CREATE TABLE exam_sessions (
    session_id UNIQUEIDENTIFIER PRIMARY KEY,
    user_id INT NOT NULL,
    exam_id INT NOT NULL,
    start_time DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    end_time DATETIME2 NULL,
    ip_address NVARCHAR(64) NOT NULL,
    device_info NVARCHAR(1024) NOT NULL,
    status NVARCHAR(20) NOT NULL,
    is_violated BIT NOT NULL DEFAULT 0,
    access_locked_at DATETIME2 NULL,
    violation_reason NVARCHAR(100) NULL
);

CREATE TABLE violation_logs (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    session_id UNIQUEIDENTIFIER NOT NULL,
    violation_type NVARCHAR(40) NOT NULL,
    [timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    action_taken NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_violation_logs_exam_sessions FOREIGN KEY (session_id) REFERENCES exam_sessions(session_id) ON DELETE CASCADE
);

CREATE TABLE monitoring_logs (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    session_id UNIQUEIDENTIFIER NOT NULL,
    event_type NVARCHAR(100) NOT NULL,
    event_time DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    details NVARCHAR(2048) NULL,
    CONSTRAINT FK_monitoring_logs_exam_sessions FOREIGN KEY (session_id) REFERENCES exam_sessions(session_id) ON DELETE CASCADE
);

CREATE TABLE exam_submissions (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    session_id UNIQUEIDENTIFIER NOT NULL UNIQUE,
    submitted_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    reason NVARCHAR(255) NOT NULL,
    mode NVARCHAR(20) NOT NULL,
    answers_snapshot NVARCHAR(MAX) NULL,
    CONSTRAINT FK_exam_submissions_exam_sessions FOREIGN KEY (session_id) REFERENCES exam_sessions(session_id) ON DELETE CASCADE
);
```

Indexes:
- `IX_exam_sessions_exam_id_user_id_status`
- `UX_exam_sessions_user_id_exam_id_active` (filtered unique on `status='Active'`)
- `IX_violation_logs_session_id_timestamp`
- `IX_monitoring_logs_session_id_event_time`
- `UX_exam_submissions_session_id`

## Backend Code Structure
- `Controllers/ExamController.cs`: API endpoints and request validation.
- `Services/IExamAntiCheatingService.cs`: anti-cheating contract.
- `Services/ExamAntiCheatingService.cs`: business logic and transaction handling.
- `Models/ExamSession.cs`: session entity + status enum.
- `Models/ViolationLog.cs`: violation log entity + violation enum.
- `Models/MonitoringLog.cs`: monitoring event entity.
- `Models/ExamSubmission.cs`: persisted submission payload.
- `Models/DTOs/ExamMonitoringDtos.cs`: request/response DTOs.
- `Data/AppDbContext.cs`: DbSets, entity mappings, relationships, and indexes.

## Production Notes
- Auto-submit path is idempotent; repeated calls do not create duplicate submissions.
- Violations are handled in DB transaction for consistency.
- Use pagination on violations endpoint for large-scale exam traffic.
- All times are stored as UTC (`GETUTCDATE()` / `DateTime.UtcNow`).
