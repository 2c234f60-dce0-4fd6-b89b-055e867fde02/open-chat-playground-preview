Working on...

1. 다음 가이드에 따라 gcloud CLI 설치
> https://cloud.google.com/sdk/docs/install?hl=ko

1. 다음 명령어를 실행하여 인증 토큰 확인
```
gcloud auth application-default print-access-token
```

> ⚡️ AccessToken은 매번 위 명령어를 통해 동적으로 가져오며, 별도의 환경변수나 설정 파일에 저장하지 않습니다. 앱 실행 시 자동으로 해당 명령어를 호출하여 최신 토큰을 사용합니다.