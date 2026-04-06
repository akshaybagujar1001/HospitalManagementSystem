# HMS Nurse Mobile App

Offline-first React Native mobile application for nurses to manage tasks and patient care.

## Project Structure

```
hms-nurse-app/
├── src/
│   ├── screens/
│   │   ├── LoginScreen.tsx
│   │   ├── TaskListScreen.tsx
│   │   ├── TaskDetailScreen.tsx
│   │   ├── PatientListScreen.tsx
│   │   └── SyncScreen.tsx
│   ├── services/
│   │   ├── api.ts
│   │   ├── syncService.ts
│   │   └── storageService.ts
│   ├── components/
│   │   ├── TaskCard.tsx
│   │   └── PatientCard.tsx
│   ├── hooks/
│   │   ├── useTasks.ts
│   │   └── useSync.ts
│   └── types/
│       └── index.ts
├── package.json
└── README.md
```

## Installation

```bash
npm install
# or
yarn install
```

## Dependencies

- @react-native-async-storage/async-storage
- @react-navigation/native
- @react-navigation/stack
- axios
- react-native

## Features

- Offline-first architecture
- Task management
- Patient information access
- Automatic sync when online
- Push notifications support

