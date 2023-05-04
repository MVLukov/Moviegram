<script>
import { useUserStore } from "@/stores/user";
import axios from "axios";

export default {
  setup() {
    const userStore = useUserStore();
    return { userStore };
  },
  data() {
    return {
      email: "",
      pass: "",
      emailErr: { error: false, errorMsg: "" },
      passErr: { error: false, errorMsg: "" },
    };
  },
  watch: {
    email(newEmail, oldEmail) {
      let regex = /^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$/gm;

      if (newEmail.length > 0) {
        if (!regex.test(newEmail)) {
          this.emailErr = { error: true, errorMsg: "Invalid email" };
        } else {
          this.emailErr = { error: false, errorMsg: "" };
        }
      } else {
        this.emailErr = { error: false, errorMsg: "" };
      }
    },
    pass(newPass, oldPass) {
      let regex = /^[\w*\?!@#%^&$*()\+=-]{4,}$/gm;

      if (newPass.length > 0) {
        if (!regex.test(newPass)) {
          this.passErr = {
            error: true,
            errorMsg: "Invalid password! (min 4 charachters)",
          };
        } else {
          this.passErr = { error: false, errorMsg: "" };
        }
      } else {
        this.passErr = { error: false, errorMsg: "" };
      }
    },
  },
  methods: {
    async submit() {
      this.emailErr = { error: false, errorMsg: "" };
      this.passErr = { error: false, errorMsg: "" };

      if (this.email.length == 0) {
        this.emailErr = { error: true, errorMsg: "Please enter an email!" };
      }

      if (this.pass.length == 0) {
        this.passErr = { error: true, errorMsg: "Please enter a password!" };
      }

      let emailErr = this.emailErr.error;
      let passErr = this.passErr.error;

      if (!emailErr && !passErr) {
        try {
          let data = JSON.stringify({ email: this.email, password: this.pass });
          let result = await axios.post("/api/Users/signin", data, {
            headers: {
              "Content-Type": "application/json",
            },
          });

          if (result.data.error) {
            if (result.data.error == "not_found") {
              this.emailErr = {
                error: true,
                errorMsg: "User with this email doesn't exists!",
              };
            }
            if (result.data.error == "invalid credentials") {
              this.passErr = {
                error: true,
                errorMsg: "Password is incorrect!",
              };
            }
          } else {
            this.userStore.setAuth(true);
            this.userStore.setUserDetails(result.data);
            this.$router.push("/");
          }
        } catch (error) {
          console.log(error);
        }
      }

      this.pass = "";
    },
  },
};
</script>

<template>
  <div class="container mt-3">
    <h1 class="text-center mb-5">Sign in in Moviegram</h1>

    <form class="w-50 mx-auto" @submit.prevent="submit">
      <div class="mb-3">
        <label for="email" class="form-label">Email address</label>
        <input
          type="email"
          :class="['form-control', { 'is-invalid': emailErr?.error }]"
          id="email"
          v-model="email"
        />
        <div class="invalid-feedback" v-if="emailErr?.error">
          {{ emailErr?.errorMsg }}
        </div>
      </div>
      <div class="mb-3">
        <label for="pass" class="form-label">Password</label>
        <input
          type="password"
          :class="['form-control', { 'is-invalid': passErr?.error }]"
          id="pass"
          v-model="pass"
        />
        <div class="invalid-feedback" v-if="passErr?.error">
          {{ passErr?.errorMsg }}
        </div>
      </div>
      <button type="submit" class="btn btn-primary" style="width: 100px">
        Sign in
      </button>
    </form>
  </div>
</template>
