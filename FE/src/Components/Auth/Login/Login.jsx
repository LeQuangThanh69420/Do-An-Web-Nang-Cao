import { useContext, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";

import { checkSession } from "../../../utils/checkSession";

import Input from "../../Shared/Input/Input";
import LogoDuRiu from "../../Shared/LogoDuRiu";
import { AppData } from "../../../Root";

import "./../../../styles/Login.css";

function Login() {

  const navigate = useNavigate();

  const {showToast, setMessage, setType, setUserData} = useContext(AppData);

  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [showModalInput, setShowModalInput] = useState('');

  const frontRef = useRef();
  const backRef = useRef();

  const handleSubmit = () => {
    let status;

    fetch(import.meta.env.VITE_API_URL +"/User/Login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        username: userName,
        password: password,
      }),
    })
      .then((response) => {
        status = response.status;
        return response.json();
      })
      .then((data) => {
        showToast();
        if (status === 200) {
          localStorage.setItem('userData', JSON.stringify(data));
          setMessage('Login Successfully!');
          setType('toast-success');
          setUserData(data);
          navigate('/');
        } else {
          setMessage(data.message)
          setType('toast-error')
        }
      })

  };

  const handleForgetPassword = () => {

  }

  const handleNavigateSignUp = () => {
    frontRef.current.style.transform = 'rotateY(180deg)';
    backRef.current.style.transform = 'rotateY(0deg)';
    setTimeout(() => {
      navigate('/sign-up')
    }, 500)
  }

  useEffect(() => {
    if(checkSession()) {
      navigate('/')
    }
  }, [])

  useEffect(() => {
    frontRef.current.style.transform = 'rotateY(180deg)';
    backRef.current.style.transform = 'rotateY(0deg)';

    setTimeout(() => {
      frontRef.current.style.transform = 'rotateY(0deg)';
      backRef.current.style.transform = 'rotateY(-180deg)';
    }, 200);
  }, [])

  return (
    <div className="login-screen">
      <div className="go-home">
        <LogoDuRiu logoColor={"#7400CC"} logoNameColor={"#7400CC"} />
      </div>
      <div className="login-container">
        <div className="bg-img"></div>
        <div className="main-form xl">
          <div className="front-card xl absolute login-card" ref={frontRef} style={{ transition: 'transform 0.5s linear' }}>
            <p className="title-login">Login</p>
            <div className="login-inputs">
              <Input
                label={"Username"}
                type="email"
                regex={/^(?!\s*$).+/}
                icon={"person"}
                errorMessage="Username can not be empty!"
                setData={setUserName}
              />
              <div className="password-container">
                <Input
                  label={"Password"}
                  type={"password"}
                  regex={/^(?!\s*$).+/}
                  icon="lock"
                  errorMessage={
                    "Password can not be empty!"
                  }
                  setData={setPassword}
                />
                <p className="links" onClick={handleForgetPassword}>Forgot Password?</p>
              </div>
            </div>
            <div className="login-button-container">
              <button className="login-button" onClick={handleSubmit}>
                Login
              </button>
              <div className="create-account">
                Don't have an account? <span className="links" onClick={handleNavigateSignUp}>Create one now</span>
              </div>
            </div>
          </div>
          <div className="back-card xl absolute" ref={backRef} style={{ transition: 'transform 0.5s linear' }}>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Login;
