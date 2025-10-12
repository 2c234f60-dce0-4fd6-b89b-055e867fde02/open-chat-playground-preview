# 멀티 ConnectorType 지원 아키텍처 설계 및 검토

## 1. 요구사항 및 배경
- 기존 구조는 단일 ConnectorType에 대해 ChatClient를 생성 및 DI 컨테이너에 등록함.
- 향후 여러 ConnectorType을 동시에 지원, 사용자가 UI에서 선택해 채팅 요청을 보낼 수 있어야 함.
- 모든 ChatClient는 미들웨어(캐시, 오픈텔레메트리 등) 체인을 동일하게 적용받아야 함.

## 2. 검토한 방안

### 1) 요청 시 팩터리 생성 및 DI 등록
- 사용자가 ConnectorType을 선택할 때마다 리플렉션 기반 팩터리로 ChatClient를 생성.
- 생성된 인스턴스를 Dictionary<ConnectorType, ChatClient> 형태로 DI 컨테이너에 등록 및 관리.
- 장점: 메모리 효율, ConnectorType 추가/변경에 유연.
- 단점: ChatClient 생성 비용 반복, 미들웨어 체인 동적 구성 필요, DI 일관성 저하, 리플렉션 유지보수 부담.

### 2) 앱 기동 시 전체 ConnectorType별 ChatClient 생성 및 DI 등록 (선택)
- 앱 시작 시 지원 가능한 모든 ConnectorType에 대해 ChatClient를 미리 생성.
- Dictionary<ConnectorType, ChatClient>로 DI 컨테이너에 등록.
- UI에서는 사용 가능한 ConnectorType만 표출, 사용자는 선택하여 요청.
- 장점: DI/미들웨어 일관성, 실시간 응답성, 코드 단순성, 확장성 우수.
- 단점: ChatClient가 무거울 경우 리소스 낭비 가능(실제 사용 빈도 고려 필요).

### 3) 단일 ChatClient 유지, ConnectorType 변경 시 교체
- 단일 ChatClient 인스턴스만 관리, 사용자가 ConnectorType을 바꿀 때마다 새로 생성해 교체.
- 단점: 상태 관리 복잡, 동시성 이슈, 미들웨어/DI 재구성 필요, 구조 복잡.

## 3. 선택 및 판단 기준
- **선택:** 2번(앱 기동 시 전체 생성 및 DI 등록, Dictionary 관리)
- **판단 기준:**
  - ChatClient 생성 비용 및 리소스 점유
  - 미들웨어 체인 일관성 및 DI 확장성
  - 실시간 응답성 및 코드 가독성
  - 실제 사용 패턴(ConnectorType 빈도)
- **보완:** ChatClient가 매우 무거운 경우, 1번(요청 시 팩터리)도 일부 병행 고려

## 4. 구현 방향 요약
- 앱 기동 시 지원 가능한 모든 ConnectorType에 대해 ChatClient를 생성, Dictionary로 DI 등록
- UI에서는 사용 가능한 ConnectorType만 표출, 사용자는 선택하여 요청
- 미들웨어(캐시, 오픈텔레메트리 등)는 기존과 동일하게 모든 ChatClient에 일관 적용
- 확장 시 ConnectorType, ChatClient, 설정, UI만 추가하면 됨

---

본 문서는 향후 아키텍처 논의 및 구현 시 기준 문서로 활용한다.