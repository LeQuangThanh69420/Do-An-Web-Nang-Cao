import './Input.css'
import { useEffect, useState } from "react"

function Input({ label, type, regex }) {
    const [inputValue, setInputValue] = useState('');
    const [error, setError] = useState('');
    const [labelStatus, setLabelStatus] = useState('');
    const [eye, setEye] = useState('close');
    const [internalType, setInternalType] = useState('');

    useEffect(() => {
        setInternalType(type);
    }, [type])

    const handleBlur = () => {
        if (!inputValue.match(regex)) {
            setError('error');
            setLabelStatus('error')
        } else {
            setError('');
            setLabelStatus('ok');
        }
    }

    const handleFocus = () => {
        setError('');
        setLabelStatus('focus')
    }

    const handleClickEye = () => {
        if(eye === 'close') {
            setEye('open');
            setInternalType('text');
        }
        else if (eye === 'open') {
            setEye('close');
            setInternalType('password');
        }
    }

    return (
        <div className={`input-container ${error}`}>
            <label htmlFor={`input-${type}`} className={`input-label ${labelStatus}`}>{label}</label>
            <input type={internalType} id={`input-${type}`} value={inputValue} className={`is${type}`} onChange={(event) => setInputValue(event.target.value)} onBlur={handleBlur} onFocus={handleFocus} />
            {type === 'password' &&
                <div className={`eye-icon ${eye}`} onClick={handleClickEye}>

                </div>}
        </div>
    )
}

export default Input