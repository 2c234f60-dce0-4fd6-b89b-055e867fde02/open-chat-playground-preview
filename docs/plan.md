## 런타임 Connector 교체 `preview-change-connector-in-runtime`

### 요구사항

현재는 AppHost에서 CLI 인자를 서로 다르게 하여 각각의 Connector별로 프로젝트를 기동
이후에는 AppHost에서는 프로젝트를 하나만 기동하고,
PlaygroundAPP 내부에서 모든 Connector에 대한 ChatClient를 미리 인플레이팅

사용자가 화면 상단 콤보박스에서 Connector를 선택하여 어떤 ChatClient를 사용할지 정할 수 있음
사용자가 선택한 Connector에 대해서 의존객체를 자동으로 받아와서 화면단에서 사용함

이러한 ChatClient는 Aspire Minimal API를 사용해 외부에 노출

다음 스텝으로는 src/OpenChat.Django에 파이썬 웹앱을 만들어 외부에서 해당 API를 사용할 것임
현재는 위와 같이 런타임에 동적으로 Connector를 선택하고, 이를 외부 API로 공개하는 데 집중

이러한 변경사항을 진행할때는 앱 로직, 테스트에만 집중하고,
Docker 컨테이너화, Azure 배포, Bicep, Github Actions에 대해서는 고려하지 말아야함.

### 완료기준

1. AppHost는 단일 인스턴스로만 기동되며, CLI 인자에 따라 별도의 Connector를 선택하지 않는다.
2. PlaygroundApp 내부에서 지원하는 모든 Connector에 대한 ChatClient가 미리 인스턴스화되어 있다.
3. UI(화면 상단 콤보박스)에서 사용자가 Connector를 선택하면, 해당 Connector의 ChatClient로 대화가 즉시 전환된다.
4. 선택된 Connector에 따라 의존 객체(설정, 인증 등)가 자동으로 주입되어 정상 동작한다.
5. 각 ChatClient는 Aspire Minimal API를 통해 외부 API로 노출된다.
6. PlaygroundApp의 API를 통해 외부에서 Connector별로 대화 기능을 사용할 수 있다.
7. 기존 CLI 인자 방식은 제거되며, Connector 추가/변경 시 재기동 없이 런타임에 반영된다.
8. 모든 변경사항은 유닛/통합 테스트로 검증된다.
9. Docker, Azure, Bicep, Github Actions 등 배포/인프라 관련 코드는 변경하지 않는다.

