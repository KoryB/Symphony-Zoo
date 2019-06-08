import TestComponent from './Components/test.vue.js'

const app = new Vue({
    el: '#app',
    data: {
        message: TestComponent,
    },

    components: {
        TestComponent,
    }
})