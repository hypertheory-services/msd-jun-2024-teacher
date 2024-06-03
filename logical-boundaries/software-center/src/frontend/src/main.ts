//in main.js
import 'primevue/resources/themes/aura-dark-noir/theme.css'
import './assets/main.css'
import './tanstack'
import 'primeflex/primeflex.css'
import 'primeicons/primeicons.css'
import VueAxios from 'vue-axios'
import axios from 'axios'
import { VueQueryPlugin } from '@tanstack/vue-query'
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ConfirmationService from 'primevue/confirmationservice'
import ToastService from 'primevue/toastservice'
import App from './App.vue'
import router from './router'
import PrimeVue from 'primevue/config'
import Ripple from 'primevue/ripple'
import DialogService from 'primevue/dialogservice'
const app = createApp(App)

import Button from 'primevue/button'
app.use(createPinia())
app.use(router)
app.use(PrimeVue, { ripple: true })
app.use(VueQueryPlugin)
app.use(ConfirmationService)
app.use(ToastService)
app.use(DialogService)
app.component('PButton', Button)
app.directive('ripple', Ripple)
app.use(VueAxios, axios)
app.mount('#app')
