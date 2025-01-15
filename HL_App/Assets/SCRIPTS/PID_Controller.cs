/// <summary>
/// The PIDController class implements a Proportional-Integral-Derivative (PID) controller,
/// which is a control loop feedback mechanism commonly used in industrial control systems.
/// This class provides functionality to control both position and angle (yaw) of a system
/// by computing and applying corrections based on the difference between a desired setpoint
/// and a measured process variable.
/// </summary>
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ Serializable ]
public class PIDController
{
    public enum DerivativeMeasurement 
    {
        Velocity,
        ErrorRateOfChange
    }

    // PID CONFIGURATION
    //
    // PID coefficients
    public float Kp;
    public float Ki;
    public float Kd;
    //
    // Target threshold + or -
    public float threshold = 0.01f;
    // Output limits
    public float output_min = -1f;
    public float output_max = 1f;
    public float integral_saturation = 1f;
    public DerivativeMeasurement derivative_measurement;
    //
    // System history
    public float value_last;
    public float error_last;
    public float integration_stored;
    public float velocity; // display field of velocity computed in controller
    public bool derivative_initialized;

    // CLASS MEMBER FUNCTIONS
    //
    public void Reset ()
    {
        derivative_initialized = false;
    }

    public float UpdatePosition ( float dt, float current_position, float target_position )
    {
        if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));
        float error = target_position - current_position;

        if ( Mathf.Abs( error ) < threshold )
        {
            integration_stored = 0;
            return 0f;
        }

        // Compute P term
        float P = Kp * error;

        // Compute I term
        integration_stored = Mathf.Clamp ( integration_stored + ( error * dt ), -integral_saturation, integral_saturation );
        float I = Ki * integration_stored;

        // Compute D term
        float d_error = ( error - error_last ) / dt;
        error_last = error;

        float d_position = ( current_position - value_last ) / dt;
        value_last = current_position;
        velocity = d_position;

        // choose D term based on initialization state
        float derivative = 0;

        if ( derivative_initialized ) {
            if ( derivative_measurement == DerivativeMeasurement.Velocity ) {
                derivative = -d_position;
            }
            else
            {
                derivative = d_error;
            }
        }
        else {
            derivative_initialized = true;
        }

        float D = Kd * derivative;

        float result = P + I + D;

        return Mathf.Clamp ( result, output_min, output_max );
    }
    //
    // Function to map angles betweem -180 and 180 [-180 - 0 - 180]
    float DeltaAngle ( float alpha, float beta) 
    {
        return ( alpha - beta + 540 ) % 360 - 180; // calculate modular difference, and remap to [-180, 180]
    }
    //
    // Yaw Angle PID controller
    public float UpdateAngle ( float dt, float current_angle, float target_angle )
    {
        if (dt <= 0) throw new ArgumentOutOfRangeException(nameof(dt));
        float error = DeltaAngle( target_angle, current_angle );

        if ( Mathf.Abs( error ) < threshold )
        {
            integration_stored = 0;
            return 0f;
        }
        // Compute P term
        float P = Kp * error;

        // Compute I term
        integration_stored = Mathf.Clamp ( integration_stored + ( error * dt ), -integral_saturation, integral_saturation );
        float I = Ki * integration_stored;

        // Compute D term
        float d_error = DeltaAngle( error, error_last ) / dt;
        error_last = error;

        float d_angle = ( current_angle - value_last ) / dt;
        value_last = current_angle;
        velocity = d_angle;

        // choose D term based on initialization state
        float derivative = 0;

        if ( derivative_initialized ) {
            if ( derivative_measurement == DerivativeMeasurement.Velocity ) {
                derivative = -d_angle;
            }
            else
            {
                derivative = d_error;
            }
        }
        else {
            derivative_initialized = true;
        }


        float D = Kd * derivative;

        float result = P + I + D;

        return Mathf.Clamp ( result, output_min, output_max );

    }
}
