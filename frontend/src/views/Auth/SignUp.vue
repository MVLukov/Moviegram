<script>
import axios from "axios";

export default {
  data() {
    return {
      username: "",
      email: "",
      pass1: "",
      pass2: "",
      usernameErr: {
        error: false,
        errorMsg: "",
      },
      emailErr: {
        error: false,
        errorMsg: "",
      },
      pass1Err: {
        error: false,
        errorMsg: "",
      },
      pass2Err: {
        error: false,
        errorMsg: "",
      },
    };
  },
  watch: {
    username(newUsername, oldUsername) {
      let regex = /^\w+$/gm;

      if (newUsername.length > 0) {
        if (!regex.test(newUsername)) {
          this.usernameErr = {
            error: true,
            errorMsg: "Invalid username! (a-zA-Z0-9_)",
          };
        } else {
          this.usernameErr = { error: false, errorMsg: "" };
        }
      } else {
        this.usernameErr = { error: false, errorMsg: "" };
      }
    },
    email(newEmail, oldEmail) {
      let regex = /^((?!\.)[\w\-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$/gm;

      if (newEmail.length > 0) {
        if (!regex.test(newEmail)) {
          this.emailErr = { error: true, errorMsg: "Invalid email!" };
        } else {
          this.emailErr = { error: false, errorMsg: "" };
        }
      } else {
        this.emailErr = { error: false, errorMsg: "" };
      }
    },
    pass1(newPass, oldPass) {
      let regex = /^[\w*\?!@#%^&$*()\+=-]{4,}$/gm;

      if (newPass.length > 0) {
        if (!regex.test(newPass)) {
          this.pass1Err = {
            error: true,
            errorMsg: "Invalid password! (min 4 charachters)",
          };
        } else {
          this.pass1Err = { error: false, errorMsg: "" };
        }
      } else {
        this.pass1Err = { error: false, errorMsg: "" };
      }
    },
    pass2(newPass, oldPass) {
      let regex = /^[\w*\?!@#%^&$*()\+=-]{4,}$/gm;

      if (newPass.length > 0) {
        if (!regex.test(newPass)) {
          this.pass2Err = {
            error: true,
            errorMsg: "Invalid password! (min 4 charachters)",
          };
        } else {
          this.pass2Err = { error: false, errorMsg: "" };
        }
      } else {
        this.pass2Err = { error: false, errorMsg: "" };
      }
    },
  },
  methods: {
    async submit() {
      this.usernameErr = { error: false, errorMsg: "" };
      this.emailErr = { error: false, errorMsg: "" };
      this.pass1Err = { error: false, errorMsg: "" };
      this.pass2Err = { error: false, errorMsg: "" };

      if (this.username.length == 0) {
        this.usernameErr = {
          error: true,
          errorMsg: "Please enter a username!",
        };
      }

      if (this.email.length == 0) {
        this.emailErr = {
          error: true,
          errorMsg: "Please enter an email address!",
        };
      }

      if (this.pass1.length == 0) {
        this.pass1Err = { error: true, errorMsg: "Please enter a password!" };
      }

      if (this.pass2.length == 0) {
        this.pass2Err = {
          error: true,
          errorMsg: "Please reenter your password!",
        };
      }

      if (
        this.pass1.length > 0 &&
        this.pass2.length > 0 &&
        this.pass1 != this.pass2
      ) {
        this.pass1Err = { error: true, errorMsg: "Password doesn't match!" };
        this.pass2Err = { error: true, errorMsg: "Password doesn't match!" };
      }

      let usernameErr = this.usernameErr.error;
      let emailErr = this.emailErr.error;
      let pass1Err = this.pass1Err.error;
      let pass2Err = this.pass2Err.error;

      if (!usernameErr && !emailErr && !pass1Err && !pass2Err) {
        try {
          let data = JSON.stringify({
            username: this.username,
            email: this.email,
            password: this.pass1,
          });
          let result = await axios.post("api/Users/signup", data, {
            headers: {
              "Content-Type": "application/json",
            },
          });

          if (result.data.error) {
            if (result.data.error == "email/username") {
              this.usernameErr = {
                error: true,
                errorMsg: "User with this username already exists!",
              };
              this.emailErr = {
                error: true,
                errorMsg: "User with this email already exists!",
              };
            } else if (result.data.error == "username") {
              this.usernameErr = {
                error: true,
                errorMsg: "User with this username already exists!",
              };
            } else if (result.data.error == "email") {
              this.emailErr = {
                error: true,
                errorMsg: "User with this email already exists!",
              };
            }
          } else {
            this.$router.push("/signin");
          }
        } catch (error) {
          console.log(error);
        }
      }

      this.pass1 = "";
      this.pass2 = "";
    },
  },
};
</script>

<template>
  <div class="container mt-3">
    <h1 class="text-center mb-5">Sign up in Moviegram</h1>

    <form class="w-50 mx-auto needs-validation" @submit.prevent="submit">
      <div class="mb-3">
        <label for="username" class="form-label">Username</label>
        <input
          type="text"
          :class="[
            'form-control',
            {
              'is-invalid': usernameErr?.error,
            },
          ]"
          id="username"
          v-model="username"
        />
        <div class="invalid-feedback" v-if="usernameErr?.error">
          {{ usernameErr?.errorMsg }}
        </div>
      </div>
      <div class="mb-3">
        <label for="email" class="form-label">Email address</label>
        <input
          type="email"
          :class="[
            'form-control',
            {
              'is-invalid': emailErr?.error,
            },
          ]"
          id="email"
          v-model="email"
        />
        <div class="invalid-feedback" v-if="emailErr?.error">
          {{ emailErr?.errorMsg }}
        </div>
      </div>
      <div class="mb-3">
        <label for="pass1" class="form-label">Password</label>
        <input
          type="password"
          :class="[
            'form-control',
            {
              'is-invalid': pass1Err?.error,
            },
          ]"
          id="pass1"
          v-model="pass1"
        />
        <div class="invalid-feedback" v-if="pass1Err?.error">
          {{ pass1Err?.errorMsg }}
        </div>
      </div>
      <div class="mb-3">
        <label for="pass2" class="form-label">Reenter password</label>
        <input
          type="password"
          :class="[
            'form-control',
            {
              'is-invalid': pass2Err?.error,
            },
          ]"
          id="pass2"
          v-model="pass2"
        />
        <div class="invalid-feedback" v-if="pass2Err?.error">
          {{ pass2Err?.errorMsg }}
        </div>
      </div>
      <button type="submit" class="btn btn-primary" style="width: 100px">
        Sign up
      </button>
    </form>
  </div>
</template>
