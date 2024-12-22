// Service worker


self.addEventListener('fetch', (ev) => {
    console.log('Service worker do some fetch');
});

self.addEventListener('message', (event) => {
    console.log(`Message received ${event}`);
    self.registration.showNotification(`${event.data}`);
});

// SERVICE WORKER STATUS

self.addEventListener('activate', (ev) => {
    console.log('Service worker activate');
});

self.addEventListener('install', (ev) => {
    console.log('Service worker install');
});
